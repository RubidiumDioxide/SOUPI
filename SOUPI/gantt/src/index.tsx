import React from 'react';
import { createComponentRoot } from './utils'; 
import GanttChart from './GanttChart';
import GanttJob from './GanttJob'; 
import './customstyle.css'; 


let root: ReturnType<typeof createComponentRoot> | null = null;
let currentJobs: GanttJob[] = [];    
let currentViewMode: string = 'Week'; 

function render() {
    if (!root) return;

    root.render(
        <React.StrictMode>
            <GanttChart
                jobs={currentJobs}
                viewMode={currentViewMode}
            />
        </React.StrictMode>
    )
} 

export function init(
    jobs: GanttJob[] = [],
    viewMode: string = 'Week'
) {
    currentJobs = [...jobs];
    currentViewMode = viewMode;

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

export function setViewMode(viewMode: string) {
    currentViewMode = viewMode
    
    render();
}

export function setJobs(jobs: GanttJob []) {
    currentJobs = jobs; 

    render();
}

export function cleanup() {
    const el = document.getElementById('react-gantt-root');
    if (el) el.innerHTML = ''; 
    root?.unmount?.();
    root = null;
}