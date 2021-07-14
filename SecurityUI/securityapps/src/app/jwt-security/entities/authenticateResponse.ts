import {ObjectStateType} from "../../../Shared/enums/enums";

export class AuthenticateResponse {
     id: number;
     firstName: string;
     lastName: string;
     username: string;
     jwtToken: string;
     refreshToken: string;
     objectState: ObjectStateType;
}
