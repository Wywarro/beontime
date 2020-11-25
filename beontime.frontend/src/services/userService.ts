import { reactive, readonly } from "vue";

import firebase from "firebase/app";

import { Locale } from "date-fns";
import { pl } from "date-fns/locale";

export interface User {
    loggedIn: boolean,
    data: firebase.User | null,
    preferences: {
        locale: Locale;
    }
}

const user = reactive<User>({
    loggedIn: false,
    data: null,
    preferences: {
        locale: pl
    }
});

const setLoggedIn = (value: boolean) => {
    user.loggedIn = value;
};

const setUser = (data: firebase.User | null) => {
    user.data = data;
};

const fetchUser = (user: firebase.User | null): void => {
    setLoggedIn(user !== null);
    if (user) {
        setUser({
            displayName: user.displayName,
            email: user.email,
        } as firebase.User);
    } else {
        setUser(null);
    }
};

export interface userService {
    user: User,
    fetchUser: (user: firebase.User | null) => void,
}

export default { user: readonly(user), fetchUser } as userService;
