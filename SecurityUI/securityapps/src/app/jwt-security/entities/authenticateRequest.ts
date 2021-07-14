import {ObjectStateType} from "../../../Shared/enums/enums";

export class AuthenticateRequest {

  username: string;
  password: string;
  objectState: ObjectStateType;
  constructor(userName: string, password: string) {
    this.username = userName;
    this.password = password;
  }

}
