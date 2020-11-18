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
import { defineComponent } from "vue";
import Topbar from "@/components/Topbar.vue";
import NavigationDrawer from "@/components/NavigationDrawer.vue";

export default defineComponent({
    name: "Home",
    components: {
        Topbar,
        NavigationDrawer,
    },
    data: () => ({
        drawerOpened: true,
    }),
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
