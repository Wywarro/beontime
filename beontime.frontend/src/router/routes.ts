import { RouteRecordRaw } from "vue-router";

import NotFound from "@/views/NotFound.vue";

import Login from "@/views/Login.vue";
import Signup from "@/views/Signup.vue";

import Home from "@/views/Home.vue";
import WorkCalendar from "@/views/WorkCalendar.vue";
import OvertimeStatus from "@/views/OvertimeStatus.vue";

const routes: Array<RouteRecordRaw> = [
    {
        path: "/",
        name: "home",
        component: Home,
    },
    {
        path: "/workdays",
        name: "work-calendar",
        component: WorkCalendar,
    },
    {
        path: "/overtime-status",
        name: "overtime-status",
        component: OvertimeStatus,
    },
    {
        path: "/accounts/login",
        name: "login",
        component: Login,
    },
    {
        path: "/accounts/signup",
        name: "signup",
        component: Signup,
    },
    {
        path: "/:pathMatch(.*)*",
        component: NotFound,
    },
];

export default routes;
