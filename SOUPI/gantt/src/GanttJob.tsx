export default interface GanttJob {
    id: string;
    name: string;
    start: string;
    end: string;
    progress: number;
    dependencies?: string[]; 
    custom_class?: string; 
}