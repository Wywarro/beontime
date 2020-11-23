import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

import { FontAwesomeIcon } from "@/plugins/font-awesome";

import "./assets/tailwind.css";
import "hamburgers/dist/hamburgers.css";

const app = createApp(App).use(store).use(router);

app.component("FontAwesomeIcon", FontAwesomeIcon);

app.mount("#app");
