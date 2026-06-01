import { useEffect, useRef, forwardRef, useImperativeHandle } from 'react';
import Gantt from 'frappe-gantt';
import '../../node_modules/frappe-gantt/dist/frappe-gantt.css';
import GanttJob from './GanttJob'; 


const GanttChart = forwardRef((props: {
    jobs: GanttJob[];
    isReadonly: boolean; 
}, ref) => {
    const containerRef = useRef<HTMLDivElement | null>(null);
    const ganttRef = useRef<any>(null);

    useImperativeHandle(ref, () => ({
        getInternalJobsForCSharp: () => { 
            const retrievedJobs: any[] = ganttRef.current ? ganttRef.current.tasks : [];

            return retrievedJobs.map(job => ({
                id: job.id,
                name: job.name,
                start: (() => {
                    const d = new Date(job.start);
                    const localDate = new Date(d.getTime() - (d.getTimezoneOffset() * 60000));
                    return localDate.toISOString().split('T')[0];
                })(), 
                end: (() => {
                    const d = new Date(job.end); 
                    const localDate = new Date(d.getTime() - (d.getTimezoneOffset() * 60000));
                    return localDate.toISOString().split('T')[0];
                })(), 
                progress: job.progress,
                dependencies: job.dependencies 
            }));
        }, 

        haveJobsChanged: (newJobs: GanttJob[]): boolean => {
            const currentJobs: GanttJob[] = ganttRef.current ? ganttRef.current.tasks : [];

            if (currentJobs.length !== newJobs.length) return true;
            
            // 2. Create a map of the current jobs for O(1) lookups
            const currentJobsMap = new Map<string, GanttJob>(
                currentJobs.map(job => [job.id, job])
            );

            // 3. Check if every new job matches the corresponding current job
            for (const newJob of newJobs) {
                const currentJob = currentJobsMap.get(newJob.id);

                // If the ID doesn't exist in current jobs, it's a new job
                if (!currentJob) { return true; }

                // Compare all relevant fields
                const isProgressChanged = Number(currentJob.progress) !== Number(newJob.progress);
                const isStartChanged = Date.parse(currentJob.start) !== Date.parse(newJob.start);
                const isEndChanged = Date.parse(currentJob.end) !== Date.parse(newJob.end);

                const hashCurrent = Array.isArray(currentJob.dependencies) ? [...currentJob.dependencies].sort().join(',') : '';
                const hashNew = Array.isArray(newJob.dependencies) ? [...newJob.dependencies].sort().join(',') : ''; 
                const isDependenciesChanged = hashCurrent !== hashNew;

                const isNameChanged = currentJob.name !== newJob.name;

                if (isNameChanged || isStartChanged || isEndChanged || isProgressChanged || isDependenciesChanged) return true; 
            }

            return false; 
        }
    }));

    useEffect(() => {
        if (containerRef.current && !ganttRef.current) {
            const GanttConstructor = (Gantt as any).default || Gantt;

            const custom_bar_height = 30; 
            const custom_padding = 10; 
            const custom_upper_header_height = 45; 
            
            ganttRef.current = new GanttConstructor(containerRef.current,
                props.jobs as GanttJob[], {
                    readonly: props.isReadonly,
                    today_button: true, 
                    view_mode: "Day", 
                    view_mode_select: false, 
                    snap_at: '1d', 
                    move_dependencies: false, 
                    scroll_to: 'start',

                    onprogress_change: function (task: GanttJob, progress: number) {
                        // Find the actual internal object reference and update its property
                        const internalJob = ganttRef.current.tasks.find((j: any) => j.id === task.id);
                        if (internalJob) {
                            internalJob.progress = progress;
                        }
                    },

                    on_date_change: function (task: GanttJob, start: Date, end: Date) {
                        // Find the actual internal object reference and update its properties
                        const internalJob = ganttRef.current.tasks.find((j: any) => j.id === task.id);
                        if (internalJob) {
                            internalJob.start = start; 
                            internalJob.end = end;
                        }
                    },

                    popup_on: "click", 
                    bar_height: custom_bar_height, 
                    padding: custom_padding, 
                    upper_header_height: custom_upper_header_height, 
                    container_height: 600, 
            });

            // right-click handler 
            const handleRightClick = async (e: MouseEvent) => {
                const taskBar = (e.target as HTMLElement).closest('.bar-wrapper');
                if (taskBar) {
                    e.preventDefault();
                    const jobId = taskBar.getAttribute('data-id');

                    if (jobId && (window as any).interop) {
                        const coords = {
                            clientX: e.clientX,
                            clientY: e.clientY
                        };

                        await (window as any).interop.callCSharpMethod('RightClickJobFromJS', jobId, coords);
                    }
                }
            }; 

            const el = containerRef.current;
            el.addEventListener('contextmenu', handleRightClick);

            ganttRef.current.show_popup = function () {
                return;
            };

            return () => {
                el.removeEventListener('contextmenu', handleRightClick);
                if (containerRef.current) containerRef.current.innerHTML = '';
                ganttRef.current = null;
            }; 
        }
    }, []);

    useEffect(() => {
        if (ganttRef.current) {
            ganttRef.current.refresh(props.jobs);
        }
    }, [props.jobs]);

    return <div ref={containerRef} className="gantt-target"></div>;
}); 

export default GanttChart; 