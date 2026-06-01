import React from 'react';
import { createComponentRoot } from './utils'; 
import GanttChart from './GanttChart';
import GanttJob from './GanttJob'; 


let root: ReturnType<typeof createComponentRoot> | null = null;
let currentJobs: GanttJob[] = [];    
let currentIsReadonly: boolean = true;
const ganttComponentRef = React.createRef<any>(); 

function render() {
    if (!root) return;

    root.render(
        <React.StrictMode>
            <GanttChart
                ref={ganttComponentRef}
                jobs={currentJobs}
                isReadonly = {currentIsReadonly}
            />
        </React.StrictMode>
    )
} 

export function init(
    jobs: GanttJob[] = [],
    isReadonly: boolean = true 
) {
    currentJobs = [...jobs];
    currentIsReadonly = isReadonly; 

    const container = document.getElementById('react-gantt-root');

    if (!container) {
        console.warn("Target container #react-gantt-root not found.");
        return;
    }

    if (root) {
        try {
            root.unmount();
        } catch (e) {
            // Root might already be partially destroyed by Blazor
        }
        root = null;
    }

    root = createComponentRoot('react-gantt-root'); 

    render();
}

export function setJobs(jobs: GanttJob []) {
    if (ganttComponentRef.current && ganttComponentRef.current.haveJobsChanged(jobs)) {
        currentJobs = jobs; 

        console.log("render from setJobs");

        render();  
    }
}

export function getJobs(): GanttJob[] {
    if (ganttComponentRef.current) { 
        return ganttComponentRef.current.getInternalJobsForCSharp(); 
    }

    return currentJobs; 
}

export function cleanup() {
    const el = document.getElementById('react-gantt-root');
    if (el) el.innerHTML = ''; 
    root?.unmount?.();
    root = null;
}