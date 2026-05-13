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
        getInternalJobs: () => { 
            const retrievedJobs: any[] = ganttRef.current ? ganttRef.current.tasks : [];

            return retrievedJobs.map(job => ({
                id: job.id,
                name: job.name,
                start: job.start,
                end: job.end,
                progress: job.progress,
                dependencies: Array.isArray(job.dependencies)
                    ? job.dependencies.join(',')
                    : (job.dependencies || "")
            }));
        }
    }));

    useEffect(() => {
        if (containerRef.current && !ganttRef.current) {
            const GanttConstructor = (Gantt as any).default || Gantt;

            const custom_bar_height = 30; 
            const custom_padding = 10; 
            const custom_upper_header_height = 45; 
            const calculated_container_height = (props.jobs.length * (custom_bar_height + custom_padding)) + custom_upper_header_height + 200; 
            const custom_container_height = (calculated_container_height > 600) ? "auto" : calculated_container_height; 

            ganttRef.current = new GanttConstructor(containerRef.current,
                props.jobs as GanttJob[], {
                    readonly: props.isReadonly,
                    today_button: true, 
                    view_mode_select: true, 
                    onprogress_change: function (task: GanttJob, progress: number) {
                        const updatedTasks = ganttRef.current.tasks.map((job : GanttJob) => {
                            if (job.id === task.id) {
                                return {
                                    ...job,
                                    progress: progress
                                };
                            }
                            return job;
                        }); 

                        ganttRef.current.tasks = updatedTasks; 
                    },
                    on_date_change: function (task: GanttJob, start: Date, end: Date) {
                        const updatedTasks = ganttRef.current.tasks.map((job: GanttJob) => {
                            if (job.id === task.id) {
                                return {
                                    ...job,
                                    start: start, 
                                    end: end 
                                };
                            }
                            return job;
                        });

                        ganttRef.current.tasks = updatedTasks; 
                    },  
                    popup_on: "hover", 
                    bar_height: custom_bar_height, 
                    padding: custom_padding, 
                    upper_header_height: custom_upper_header_height, 
                    container_height: custom_container_height 
            });

            // left-click handler 
            const handleLeftClick = async (e: MouseEvent) => {
                const taskBar = (e.target as HTMLElement).closest('.bar-wrapper');
                if (taskBar) {
                    e.preventDefault();
                    const jobId = taskBar.getAttribute('data-id');

                    if (jobId && (window as any).interop) {
                        const coords = {
                            clientX: e.clientX,
                            clientY: e.clientY
                        };

                        await (window as any).interop.callCSharpMethod('LeftClickJobFromJS', jobId, coords);
                    }
                }
            };

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
            el.addEventListener('click', handleLeftClick);

            return () => {
                el.removeEventListener('contextmenu', handleRightClick);
                el.removeEventListener('click', handleLeftClick);
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