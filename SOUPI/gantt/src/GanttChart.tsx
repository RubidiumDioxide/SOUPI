import React, { useEffect, useRef } from 'react';
import Gantt from 'frappe-gantt';
import '../../node_modules/frappe-gantt/dist/frappe-gantt.css';
import GanttJob from './GanttJob'; 


export default function GanttChart(props: {
    jobs: GanttJob[];
    viewMode: string;
}) {
    const containerRef = useRef<HTMLDivElement | null>(null); 
    const ganttRef = useRef<any>(null);  

    useEffect(() => {
        if (containerRef.current && !ganttRef.current) {
            const GanttConstructor = (Gantt as any).default || Gantt;

            ganttRef.current = new GanttConstructor(containerRef.current, props.jobs, {
                view_mode: props.viewMode,
                on_click: async (job: any) => {
                    await (window as any).interop?.callCSharpMethod('SelectJobFromJS', job.id);
                }
            });
        }

        return () => {
            if (containerRef.current) containerRef.current.innerHTML = '';
            ganttRef.current = null;
        };
    }, []);

    useEffect(() => {
        if (ganttRef.current) {
            ganttRef.current.refresh(props.jobs);
        }
    }, [props.jobs]);

    useEffect(() => {
        if (ganttRef.current) {
            ganttRef.current.change_view_mode(props.viewMode);
        }
    }, [props.viewMode]);

    return <div ref={containerRef} className="gantt-target"></div>;
}

