import { reactive, readonly } from "vue";

import firebase from "firebase";

interface User {
    loggedIn: boolean,
    data: firebase.User | null
}

const user = reactive<User>({
    loggedIn: false,
    data: null,
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

export default { user: readonly(user), fetchUser };
