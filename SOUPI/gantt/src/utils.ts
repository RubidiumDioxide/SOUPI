import { createRoot } from 'react-dom/client';

export function createComponentRoot(containerId: string = 'root') {
    const container = document.getElementById(containerId);
    if (!container) {
        throw new Error(`React container #${containerId} not found`);
    }
    const root = createRoot(container);
    return root;
}