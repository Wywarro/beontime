import { container } from "inversify-props";
import DateService from "./services/DateService";
import IDateService from "./services/IDateService";

export default function buildDependencyContainer(): void {
  container.addTransient<IDateService>(DateService);
}
