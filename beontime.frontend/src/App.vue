<template>
  <Topbar v-model:drawer-opened="drawerOpened" />
  <NavigationDrawer v-model:drawer-opened="drawerOpened" />
  <!-- <router-view/> -->
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

</style>
