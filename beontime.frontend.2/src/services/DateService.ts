import { injectable } from "inversify-props";

export interface IDateService {
  getNow: () => Date;
}

@injectable()
export default class DateService implements IDateService {
  getNow(): Date {
    return new Date();
  }
}
