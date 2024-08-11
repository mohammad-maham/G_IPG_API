export default class tokenInfo {
  constructor(
    public terminalId: number,
    public resNum: number,
    public status: number,
    public errorCode: number,
    public errorDesc: string,
    public token: string,
    public type: string,
    public responseCode: string
  ) { }
}
