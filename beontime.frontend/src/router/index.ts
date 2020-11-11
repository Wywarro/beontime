import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Home from "@/views/Home.vue";
import WorkCalendar from "@/views/WorkCalendar.vue";
import OvertimeStatus from "@/views/OvertimeStatus.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
  {
    path: "/",
    name: "dajesz",
    component: WorkCalendar,
  },
  {
    path: "/",
    name: "no jasne",
    component: OvertimeStatus,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
