export interface Message{
    id: number;
    idUser: string;
    text: string;
    img: string;
    answerText: string;
    view: boolean;
    send: boolean
    updatedAt: Date;
}

export interface MessageData{
    message: Array<Message>;
}