<template>
<div class="cal-container">
  <div class="cal-title">Hello!</div>
  <div class="cal-days">
    <div></div>
    <div></div>
    <div class="cal-day">{{ getMonday(new Date()) }}</div>
    <div class="cal-day">Mon</div>
    <div class="cal-day">Mon</div>
    <div class="cal-day">Mon</div>
    <div class="cal-day">Mon</div>
    <div class="cal-day">Mon</div>
    <div class="cal-day">Mon</div>
  </div>
  <div class="cal-content">
    <div
      v-for="(hour, index) in hours"
      :key="hour"
      class="cal-time"
      :style="{'grid-row': index + 1}"
    >{{ hourTime(hour) }}</div>
  </div>
  <div></div>
</div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";

export default defineComponent({
  setup () {
    const hours = ref([
      ...Array(25).keys(),
    ]);

    const hourTime = (number: number) => {
      if (number < 10) {
        return `0${number}:00`;
      }
      return `${number}:00`;
    };

    const getMonday = (date: Date) => {
      date = new Date(date);
      const dayOfWeek = date.getDay();
      const sundayAdjustment = dayOfWeek === 0 ? -6 : 1;
      const diff = date.getDate() - dayOfWeek + sundayAdjustment; // adjust when day is sunday

      const monday = new Date(date.setDate(diff));

      const options = { weekday: "long", month: "long", day: "numeric", };
      return monday.toLocaleDateString("pl-PL", options);
    };

    return { hours, hourTime, getMonday, };
  },
});
</script>

<style lang="less" scoped>
@title-height: 3em;
@days-height: 3em;
@time-width: 3em;
@time-height: 3em;
@grid-color: #dadce0;
@calendar-template: @time-width 10px repeat(7, 1fr);
@current-time-color: #ea4335;

.cal {
  &-container {
    width: 100%;
    display: grid;
    grid-template-rows: @title-height @days-height auto;
  }

  &-title {
    background: #217346;
    text-align: center;
    display: grid;
    place-content: center;
    color: #fff;
    top: 0;
    z-index: 10;
  }

  &-days {
    background: #f3f2f1;
    display: grid;
    place-content: center;
    text-align: center;
    grid-template-columns: @calendar-template;
    top: @title-height;
    z-index: 10;
    border-bottom: 2px solid @grid-color;
  }

  &-day {
    border-left: 1px solid @grid-color;
  }

  &-content {
    display: grid;
    grid-template-columns: @calendar-template;
    grid-template-rows: repeat(24, @time-height);
  }

  &-time {
    grid-column: 1;
    text-align: right;
    align-self: end;
    font-size: 80%;
    position: relative;
    bottom: -1ex;
    color: #70757a;
    padding-right: 2px;
  }
}
</style>
