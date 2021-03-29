import "reflect-metadata";
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import buildDependencyContainer from "./app.container";

import { FontAwesomeIcon } from "@/plugins/font-awesome";
import BeButton from "@/libs/BeButton.vue";

import "@/assets/tailwind.css";
import "hamburgers/dist/hamburgers.css";

import "./firebaseInit";

buildDependencyContainer();

const app = createApp(App).use(store).use(router);

app.component("FontAwesomeIcon", FontAwesomeIcon);
app.component("BeButton", BeButton);

app.config.warnHandler = (msg, vm, trace) => {
  if (process.env.NODE_ENV === "development") {
    // eslint-disable-next-line no-console
    console.warn({ msg, vm, trace });
  }
};

app.config.errorHandler = (err, vm, info) => {
  if (process.env.NODE_ENV === "development") {
    // eslint-disable-next-line no-console
    console.error({ err, vm, info });
  }
};

router.isReady().then(() => {
  app.mount("#app");
});
