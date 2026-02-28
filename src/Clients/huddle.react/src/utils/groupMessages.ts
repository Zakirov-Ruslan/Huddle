import type { MessageDto } from "../api/types";

const groupByDay = (messages: MessageDto[]) => {
    const groups: { day: string; messages: MessageDto[] }[] = [];
    let currentDay = '';
    let currentGroup: MessageDto[] = [];

    for (const msg of messages) {
        const msgDay = new Date(msg.sentAt).toLocaleDateString('ru-RU');

        if (msgDay !== currentDay) {
            if (currentGroup.length > 0) {
                groups.push({ day: currentDay, messages: currentGroup });
            }
            currentDay = msgDay;
            currentGroup = [msg];
        } else {
            currentGroup.push(msg);
        }
    }

    if (currentGroup.length > 0) {
        groups.push({ day: currentDay, messages: currentGroup });
    }

    return groups;
};

const groupConsecutiveByAuthor = (messages: MessageDto[]) => {
    const groups: { authorId: string; messages: MessageDto[] }[] = [];
    let currentAuthor = '';
    let currentGroup: MessageDto[] = [];

    for (const msg of messages) {
        if (msg.authorId !== currentAuthor) {
            if (currentGroup.length > 0) {
                groups.push({ authorId: currentAuthor, messages: currentGroup });
            }
            currentAuthor = msg.authorId;
            currentGroup = [msg];
        } else {
            currentGroup.push(msg);
        }
    }

    if (currentGroup.length > 0) {
        groups.push({ authorId: currentAuthor, messages: currentGroup });
    }

    return groups;
};

export const groupMessagesByDayAndAuthor = (messages: MessageDto[]) => {
    const sorted = [...messages].sort((a, b) =>
        new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
    );

    const dayGroups = groupByDay(sorted);
    return dayGroups.map(dayGroup => ({
        ...dayGroup,
        authorGroups: groupConsecutiveByAuthor(dayGroup.messages),
    }));
};