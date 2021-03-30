import firebase from "firebase/app";
import User from "@/types/User";

export default interface IUserService {
  user: User;
  fetchUser: (user: firebase.User | null) => void;
  setToken: () => Promise<void>;
  getToken: string;
  isAuthenticated: () => Promise<firebase.User | null>;
}
