export function adjustHeight(element: HTMLTextAreaElement): void {
    element.style.height = 'inherit'; 
    element.style.height = `${element.scrollHeight}px`;
}