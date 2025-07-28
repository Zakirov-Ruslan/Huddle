export function adjustHeight(event: React.FormEvent<HTMLTextAreaElement>): void {
    event.currentTarget.style.height = "inherit";
    event.currentTarget.style.height = `${event.currentTarget.scrollHeight}px`;
}