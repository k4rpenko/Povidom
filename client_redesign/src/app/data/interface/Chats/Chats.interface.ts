import { User } from "../Users/User.interface"
import { Message } from "./Message.interface"

export interface Chats{
  id?: string
  AddUsersIdChat?: Array<string>
  user?: User
  message?: Message
  messageArray?: Array<Message>
}
