export default interface IDateService {
  format: (date: Date, formatTokens: string) => string;
  formatDistanceToNow: (date: Date) => string;
  startOfWeek: (date: Date) => Date;
  endOfWeek: (date: Date) => Date;
  getNow: () => Date;
}
