<template>
    <div>
        <header>
            <Topbar v-model:drawer-opened="drawerOpened" />
        </header>
        <NavigationDrawer v-model:drawer-opened="drawerOpened" />
        <main
            class="transform transition-all duration-300 pt-16 p-4"
            :class="{ 'ml-64': drawerOpened }"
        >
            <router-view />
        </main>
    </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, provide } from "vue";

import Topbar from "@/components/Topbar.vue";
import NavigationDrawer from "@/components/NavigationDrawer.vue";

import firebase from "firebase/app";
import "firebase/auth";

import userService from "@/services/userService";
import dateService from "@/services/dateService";

export default defineComponent({
    name: "App",
    components: {
        Topbar,
        NavigationDrawer,
    },
    setup() {
        onMounted(() => {
            firebase.auth().onAuthStateChanged((user: firebase.User | null) => {
                userService.fetchUser(user);
            });
        });

        provide("userService", userService);
        provide("dateService", dateService);

        const drawerOpened = ref(true);
        return { drawerOpened };
    }
});
</script>

<style lang="less">
body {
    font-family: "Lato", sans-serif;
}

* {
    box-sizing: border-box;
}
</style>
