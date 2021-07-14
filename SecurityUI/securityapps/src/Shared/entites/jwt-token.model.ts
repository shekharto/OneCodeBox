export interface JwtTokenModel {
  unique_name: string;
  nameid: string;
  role: string;
  clientDb: string;
  clientId: string;
  iss: string;
  TokenType: string;
  Rft: string;
  tasks: string;
  AuthFrom: string;
  nbf: number;
  exp: number;
  iat: number;
  test: string;
  Exs: string;
}
