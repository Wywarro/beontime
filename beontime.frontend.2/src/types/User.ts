import firebase from "firebase/app";

import { Locale } from "date-fns";

export default interface User {
  data: firebase.User | null;
  token: string;
  preferences: {
    locale: Locale;
  };
}
