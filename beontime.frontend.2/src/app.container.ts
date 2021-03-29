import { container } from "inversify-props";
import DateService from "./services/DateService";
import IDateService from "./services/IDateService";

import IUserService from "./services/IUserService";
import UserService from "./services/UserService";

export default function buildDependencyContainer(): void {
  container.addTransient<IDateService>(DateService);
  container.addSingleton<IUserService>(UserService);
}
