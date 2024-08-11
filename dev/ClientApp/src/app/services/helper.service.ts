import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import * as moment from "jalali-moment";

@Injectable({
  providedIn: "root",
})
export class HelperService {
  constructor() { }

  convertDatepickerInputToValidDate(input: any) {
    let result: any = null;
    if (!input["_isValid"]) {
      const locale = input.toString().indexOf("T") > -1 ? "en" : "fa";
      const date = moment
        .from(input, locale, "YYYY-MM-DD")
        .format("YYYY/MM/DD");
      result = date;
    } else {
      const date = moment
        .from((input as any).format("YYYY/MM/DD"), "fa", "YYYY-MM-DD")
        .format("YYYY/MM/DD");
      result = date;
    }
    return result;
  }

  getPersianDateWeek(date) {
    if (!date) return "";
    if (!date["_isValid"] && date.length < 11) return date;
    const str = moment(date).locale("fa").format("[امروز] dddd, Do MMMM YYYY");
    return str;
  }

  convertDatepickerInputToValidDateTime(input: any) {
    let result: any = null;
    if (!input["_isValid"]) {
      const locale = input.toString().indexOf("T") > -1 ? "en" : "fa";
      const date = moment
        .from(input, locale, "YYYY-MM-DD HH:mm")
        .format("YYYY/MM/DD HH:mm");
      result = date;
    } else {
      const date = moment
        .from((input as any).format("YYYY/MM/DD HH:mm"), "fa", "YYYY-MM-DD HH:mm")
        .format("YYYY/MM/DD HH:mm");
      result = date;
    }
    return result;
  }

  getPersianDate(date) {
    if (!date) return "";
    if (!date["_isValid"] && date.length < 11) return date;
    const str = moment(date).locale("fa").format("YYYY/MM/DD");
    return str;
  }

  currentPageReportTemplate(dt: any, totalRecords: number) {
    if (!totalRecords) {
      totalRecords = 0;
    }
    return (
      dt.first +
      1 +
      " - " +
      ((dt.first + dt.rows > totalRecords
        ? totalRecords
        : dt.first + dt.rows) || 0) +
      " از " +
      totalRecords
    );
  }

  getPropByString(obj, propString) {
    if (!propString) return obj;

    var prop,
      props = propString.split(".");

    for (var i = 0, iLen = props.length - 1; i < iLen; i++) {
      prop = props[i];

      var candidate = obj[prop];
      if (candidate !== undefined) {
        obj = candidate;
      } else {
        break;
      }
    }
    return obj[props[i]];
  }

  toBase64(file) {
    return new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result.toString());
      reader.onerror = (error) => reject(error);
    });
  }

  isValidIranianNationalCode(input: string): boolean {
    if (!input) return false;
    if (!/^\d{10}$/.test(input)) return false;
    if (input == '1111111111' || input == '2222222222' || input == '3333333333' || input == '4444444444' ||
      input == '5555555555' || input == '6666666666' || input == '7777777777' || input == '8888888888' ||
      input == '9999999999') {
      return false;
    }
    let check = +input[9];
    let sum = 0;
    let i;
    for (i = 0; i < 9; ++i) {
      sum += +input[i] * (10 - i);
    }
    sum %= 11;
    return (sum < 2 && check == sum) || (sum >= 2 && check + sum == 11);
  }

  checkShenaseMeli(code: string): boolean {

    var L = code.length;

    if (L < 11 || parseInt(code, 10) == 0) return false;

    if (parseInt(code.substr(3, 6), 10) == 0) return false;
    var c = parseInt(code.substr(10, 1), 10);
    var d = parseInt(code.substr(9, 1), 10) + 2;
    var z = new Array(29, 27, 23, 19, 17);
    var s = 0;
    for (var i = 0; i < 10; i++)
      s += (d + parseInt(code.substr(i, 1), 10)) * z[i % 5];
    s = s % 11; if (s == 10) s = 0;
    return (c == s);

  }

  lightenDarkenColor(col, amt) {
    var usePound = false;

    if (col[0] == "#") {
      col = col.slice(1);
      usePound = true;
    }

    var num = parseInt(col, 16);

    var r = (num >> 16) + amt;

    if (r > 255) r = 255;
    else if (r < 0) r = 0;

    var b = ((num >> 8) & 0x00ff) + amt;

    if (b > 255) b = 255;
    else if (b < 0) b = 0;

    var g = (num & 0x0000ff) + amt;

    if (g > 255) g = 255;
    else if (g < 0) g = 0;

    return (usePound ? "#" : "") + (g | (b << 8) | (r << 16)).toString(16);
  }

  getStatusTitle(item: any) {

    switch (item.State) {
      case 0:
        return "-";
      case 1:
        return "ثبت اولیه";
      case 2:
        return "در انتظار تایید";
      case 3:
        return "در انتظار پرداخت";
      case 4:
        return "عدم تایید";
      case 5:
        return "در انتظار انتشار";
      case 6:
        return "منتشر شده";
      case 10:
        return "در انتظار حذف";
      case 11:
        return "حذف شده";
      case 20:
        return "در انتظار غیرفعال شدن";
      case 21:
        return "غیرفعال";
      case 22:
        return "تایید شده";
    }
  }

  showError(err: HttpErrorResponse): string {
    if (err.error != null && err.error.message != undefined) {
      if (
        err.error.InnerException &&
        err.error.InnerException.InnerException &&
        err.error.InnerException.InnerException.ExceptionMessage.indexOf(
          "unique constraint"
        ) > -1
      ) {
        return "اطلاعات وارد شده در سیستم موجود میباشد";
      }
      else
        return err.error.message;
    }
    else {
      switch (err.status) {
        case 500:
          return err.error.ExceptionMessage;
        case 400:
          if (err.error.ResultMessage)
            return err.error.ResultMessage;
          else
            return err.error.error;
        case 0:
          return "خطای نامشخص رخ داده است";
        default:
          return err.error.error;
          ;
      }
    }
  }
}
