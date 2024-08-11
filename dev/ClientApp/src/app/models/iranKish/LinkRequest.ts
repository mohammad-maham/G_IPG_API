import FactorDetail from '../FactorDetail.Model';

export default class LinkRequest {
  constructor(
    public terminalId: number,
    public MID: number,
    public State: string,
    public InsDate: string,

    public Status: number,

    public title: string,

    public price: number,

    public orderId: string,

    public factorDetailJson: string

  ) { }
}
