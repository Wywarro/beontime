import firebase from "firebase/app";

export default interface IUserService {
  user: User;
  fetchUser: (user: firebase.User | null) => void;
  setToken: () => Promise<void>;
  getToken: string;
  isAuthenticated: () => Promise<firebase.User | null>;
}
