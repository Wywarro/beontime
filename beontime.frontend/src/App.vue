<template>
  <Topbar v-model:drawer-opened="drawerOpened" />
  <NavigationDrawer v-model:drawer-opened="drawerOpened" />
  <main
    class="transform transition-all duration-300"
    :class="{ 'ml-64': drawerOpened }"
  >
    <p>pozdro z mostu</p>
    <router-view/>
  </main>
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
  font-family: 'Lato', sans-serif;
}

main {
  @apply p-4;
}
</style>
