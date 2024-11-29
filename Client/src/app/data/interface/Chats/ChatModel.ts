
export interface ChatModel{
    IdChat?: string;
    CreatorId?: string; 
    AddUsersIdChat?: Array<string>;
    Text?: string;
    Img?: string;
    Answer?: string;
    CreatedAt?: Date
}