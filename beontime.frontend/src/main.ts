import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import { FontAwesomeIcon } from "@/plugins/font-awesome";

import "./assets/tailwind.css";
import "hamburgers/dist/hamburgers.css";

import "./firebaseInit";
import firebase from "firebase";

firebase.auth().onAuthStateChanged((user) => {
    store.dispatch("fetchUser", user);
});

const app = createApp(App).use(store).use(router);

app.component("FontAwesomeIcon", FontAwesomeIcon);

app.mount("#app");
