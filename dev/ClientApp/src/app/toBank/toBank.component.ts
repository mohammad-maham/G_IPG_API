import { Component, OnInit, ViewChild } from '@angular/core';
import "print-this";
import { ActivatedRoute } from '@angular/router';
import tokenInfo from '../models/saman/tokenInfo';
import FactorDetail from '../models/FactorDetail.Model';
import ConfirmInfo from '../models/iranKish/ConfirmInfo';

import LinkRequest from '../models/iranKish/LinkRequest';

import { HelperService } from '../services/helper.service';
import { PaymentService } from '../services/payment.service';
import { HttpErrorResponse } from '@angular/common/http';

declare var $;

@Component({
  selector: 'toBank',
  templateUrl: 'toBank.component.html',
  styleUrls: ['toBank.component.css']
})

export class ToBankComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private helperService: HelperService,
    private paymentService: PaymentService
  ) { }

  @ViewChild('FormElement', { static: false }) elmForm;
  public tknInf: any;
  public lnkReq: LinkRequest;
  public factorDetail: FactorDetail;
  public confirmInfo: ConfirmInfo;
  public Msg: string = " لطفا منتظر بمانید...";
  ngOnInit() {
    this.route.params.subscribe((x) => {
      if (x.id) {
        this.paymentService.getToken(x.id).subscribe(
          (data) => {
            debugger;
            console.log(data);
            this.tknInf = data;
            debugger;
            if (this.tknInf.responseCode == "00") {
              debugger;
              this.paymentService.GetLinkRequestDetail(x.id).subscribe(
                (data) => {
                  console.log(data);
                  this.lnkReq = data;
                  debugger;
                  this.factorDetail = JSON.parse(this.lnkReq.factorDetailJson);
                  this.paymentService.GetBankResult(x.id).subscribe(
                    (data) => {
                      this.confirmInfo = data;
                      debugger;
                    });
                  debugger;
                });
            }
          },
          (err: HttpErrorResponse) => {
            debugger;
            if (err.message != undefined)
              this.Msg = err.error;
            else
              this.Msg = "خطای ناشناخته";
          });
      }
    });
  }

  getPersianDate(date) {
    return this.helperService.getPersianDate(date);
  }

  printPreFactor() {
    debugger;
    $("#prefactorContainer").printThis({
      importCSS: true,
      importStyle: true,
      removeInline: false     
    });
  }
  getToday() {
    return this.helperService.getPersianDate(new Date());
  }
}
