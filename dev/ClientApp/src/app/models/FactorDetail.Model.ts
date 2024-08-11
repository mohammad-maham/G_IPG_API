export default class FactorDetail {
  constructor(
    public Header: FactorHeader,
    public Items: FactorItem[],
    public Footer: FactorFooter
  ) { }
}

export class FactorHeader {
  public FactorId: number;
  public FactorStatus: number;
  public FactorTitle: string;
  public FactorDescription: string;
  public CutomerName: string;
  public CustomerId: number;
  public CustomerTelephone: string;
  public CustomerMobile: string;
  public CreateDate: string;
  public PaymentDetail: PayDetail;
}

export class PayDetail {
  public PaymentDescription: string;
  public PaymentStatus: number;
  public TransactionId: number;
  public PaymentDate: string;

}
export class FactorItem {
  public ItemTitle: string;
  public ItemUnitPrice: number;
  public  ItemCount: number;
  public ItemUnitType: string;
  public ItemDiscount: number;
  public  ItemSumPrice: number;
  public ItemDesctription: string;
    }
export class FactorFooter {
  public FactorVAT: number;
  public FactorSumPrice: number;
  public FactorSumPriceByVAT: number;
  public SellerName: string;
  public SellerTelephone: string[];
  public SellerAddress: string[];
    }
