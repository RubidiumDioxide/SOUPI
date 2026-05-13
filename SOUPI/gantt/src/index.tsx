import React from 'react';
import { createComponentRoot } from './utils'; 
import GanttChart from './GanttChart';
import GanttJob from './GanttJob'; 
import './customstyle.css'; 


let root: ReturnType<typeof createComponentRoot> | null = null;
let currentJobs: GanttJob[] = [];    
let currentIsReadonly: boolean = true; 
let currentIsDarkMode: boolean = false; 
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
    isReadonly: boolean = true, 
    isDarkMode: boolean = false  
) {
    currentJobs = [...jobs];
    currentIsReadonly = isReadonly; 
    currentIsDarkMode = isDarkMode; 

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
    currentJobs = jobs; 

    render();
}

export function getJobs(): GanttJob[] {
    if (ganttComponentRef.current) {
        return ganttComponentRef.current.getInternalJobs();
    }
    return currentJobs; 
}

export function setIsDarkMode(isDarkMode: boolean) {
    const container = document.getElementById('react-gantt-root');
    if (!container) {
        console.warn("Target container #react-gantt-root not found.");
        return;
    }

    const ganttContainer = container.querySelector('.gantt-container');
    if (ganttContainer) {
        if (isDarkMode) { 
            ganttContainer.classList.add('dark-theme');
        } else {
            ganttContainer.classList.remove('dark-theme');
        }
        currentIsDarkMode = isDarkMode; 

        render();
    }
}

export function cleanup() {
    const el = document.getElementById('react-gantt-root');
    if (el) el.innerHTML = ''; 
    root?.unmount?.();
    root = null;
}