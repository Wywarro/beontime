import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
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
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
