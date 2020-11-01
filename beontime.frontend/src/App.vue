<template>
  <Topbar />
  <router-view/>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Topbar from "@/components/Topbar.vue";

export default defineComponent({
  name: "Home",
  components: {
    Topbar,
  },
});
</script>

<style lang="less">

</style>
