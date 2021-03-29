import { reactive } from "vue";
import firebase from "firebase/app";
import { injectable } from "inversify-props";
import { pl } from "date-fns/locale";
import IUserService from "./IUserService";
import User from "@/types/User";


@injectable()
export default class UserService implements IUserService {
  user = reactive<User>({
    data: null,
    token: "",
    preferences: {
      locale: pl,
    },
  });

  setUser(data: firebase.User | null): void {
    this.user.data = data;
  }

  async fetchUser(user: firebase.User | null): Promise<void> {
    if (user) {
      this.setUser({
        displayName: user.displayName,
        email: user.email,
      } as firebase.User);

      await this.setToken();
    } else {
      this.setUser(null);
    }
  }

  async setToken(): Promise<void> {
    const token = (await firebase.auth().currentUser?.getIdToken()) ?? "";
    this.user.token = token;
  }

  get getToken(): string {
    return this.user.token;
  }

  async isAuthenticated(): Promise<firebase.User | null> {
    return new Promise((resolve, reject) => {
      const unsubscribe = firebase.auth().onAuthStateChanged((user) => {
        unsubscribe();
        resolve(user);
      }, reject);
    });
  }
}
