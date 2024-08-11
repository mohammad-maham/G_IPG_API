import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import bankInfo from '../models/saman/bankInfo';

@Component({
  selector: 'fromBank',
  templateUrl: 'fromBank.component.html'
})

export class fromBankComponent implements OnInit {

  constructor(
    private route: ActivatedRoute
  ) { }

  public bnkInf: bankInfo;

  ngOnInit() {
    debugger
    this.route.params.subscribe((x) => {
      if (x.id && !isNaN(x.id)) {
        this.bnkInf.MID = x.MID;
      }
    });
  }
}
