<template>
  <div class="calendar-navigator__container">
    <div class="calendar-navigator__title">{{ month }}</div>
    <div class="calendar-navigator__days"></div>
  </div>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";

@Options({
  props: {
    month: {
      type: Number,
      required: true
    }
  }
})
export default class CalendarNavigator extends Vue {}
</script>

<style lang="less" scoped>
.calendar-navigator {
  &__container {
    height: 30vh;
    width: 40%;
    border: 2px solid red;
  }

  &__days {
    display: grid;
    padding: 20px;
    grid-template-columns: repeat(auto-fit, minmax(25px, 1fr));
    gap: 10px;
  }
}
</style>
