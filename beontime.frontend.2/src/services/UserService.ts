import { reactive } from "vue";

import firebase from "firebase/app";

import { injectable } from "inversify-props";

import { Locale } from "date-fns";
import { pl } from "date-fns/locale";

export interface User {
  data: firebase.User | null;
  token: string;
  preferences: {
    locale: Locale;
  };
}

export interface IUserService {
  user: User;
  fetchUser: (user: firebase.User | null) => void;
  setToken: () => Promise<void>;
  getToken: string;
  isAuthenticated: () => Promise<firebase.User | null>;
}

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
