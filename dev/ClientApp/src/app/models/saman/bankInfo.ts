export default class bankInfo {
  constructor(
    public terminalId: number,
    public MID: number,
    public State: string,
    public Status: number,
    public RRN: string,
    public RefNum: string,
    public ResNum: number,
    public TerminalId: number,
    public TraceNo: string,
    public Amount: number,
    public Wage: number,
    public SecurePan: string
  ) { }
}
