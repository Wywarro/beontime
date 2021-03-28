import { injectable } from "inversify-props";
import IDateService from "./IDateService";

import { pl } from "date-fns/locale";
import {
  format,
  formatDistanceToNow,
  startOfWeek,
  endOfWeek,
  Locale,
} from "date-fns";

@injectable()
export default class DateService implements IDateService {
  locale: Locale = pl;

  format(date: Date, formatTokens = "do MMMM yyyy"): string {
    return format(date, formatTokens, { locale: this.locale });
  }

  formatDistanceToNow(date: Date): string {
    return formatDistanceToNow(date, { locale: this.locale });
  }

  startOfWeek(date: Date): Date {
    return startOfWeek(date, { locale: this.locale });
  }

  endOfWeek(date: Date): Date {
    return endOfWeek(date, { locale: this.locale });
  }

  getNow(): Date {
    return new Date();
  }
}
