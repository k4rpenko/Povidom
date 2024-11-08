export interface ChatModel{
    IdChat?: string;
    CreatorId?: Date; 
    AddUsersIdChat?: Array<string>;
    Text?: string;
    Img?: string;
    Answer?: string;
}