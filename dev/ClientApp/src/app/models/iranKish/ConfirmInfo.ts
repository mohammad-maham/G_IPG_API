
export default class ConfirmInfo {
  constructor(
    public responseCode?: string,
    public status?: boolean,
    public result?: ConfirmResult
  ) { }
}

export class ConfirmResult {
  public responseCode?: string;
  public systemTraceAuditNumber?: number;
  public retrievalReferenceNumber?: number;
  public transactionDateTime?: string;
  public transactionDate?: string;
  public transactionTime?: string;
  public processCode?: string;
  public requestId?: number;
  public additional?: string;
  public billType?: string;
  public billId?: number;
  public amount?: number;
  public revertUri?: string;
  public acceptorId?: number;
  public terminalId?: number;
  public tokenIdentity?: string;
  public isVerified?: boolean;
  public isMultiplex?: boolean;
  public isReversed?: boolean;
  public maskedPan?: string;
  public description?: string;
  public status?: boolean;
  public result?: string;


}


