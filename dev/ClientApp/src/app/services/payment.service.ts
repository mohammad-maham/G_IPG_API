import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class PaymentService {

  constructor(
    private httpClient: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) { }

  getToken(guid): Observable<any> {
    return this.httpClient.get<any>(
      `${this.baseUrl}Pay/GetToken?guid=` + guid
    );
  }

  GetLinkRequestDetail(guid): Observable<any> {
    return this.httpClient.get<any>(
      `${this.baseUrl}Pay/GetLinkRequestDetail?guid=` + guid
    );
  }

  GetBankResult(guid): Observable<any> {
    return this.httpClient.get<any>(
      `${this.baseUrl}Pay/GetBankResult?guid=` + guid
    );
  }
}
