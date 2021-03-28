<template>
  <div>
    <div class="cal__container">
      <div class="cal__header">
        <font-awesome-icon
          icon="chevron-circle-left"
          data-cy="substractWeekButton"
          class="cal__nav-icon"
          @click="subtractOneWeek"
        />

        <div class="cal__title">
          {{ showMonth[0].toUpperCase() + showMonth.slice(1) }}
        </div>

        <BeButton
          class="cal__today"
          color="secondary"
          data-cy="todayButton"
          @click="setToday"
          >Today</BeButton
        >

        <font-awesome-icon
          icon="chevron-circle-right"
          data-cy="addWeekButton"
          class="cal__nav-icon"
          @click="addOneWeek"
        />
      </div>
      <div class="cal__days">
        <div />
        <div />
        <div v-for="day in days" :key="`30${day}`" class="cal__day">
          {{ formatDate(getDayFromMonday(day), dayFormatTokens) }}
        </div>
      </div>
      <div class="cal__content">
        <div
          v-for="hour in hours"
          :key="hour"
          class="cal__time"
          :style="{ 'grid-row': hour }"
        >
          {{ hourTime(hour) }}
        </div>
        <div class="cal__filler-col" />
        <div
          v-for="day in days"
          :key="`10${day}`"
          class="cal__col"
          :style="{ 'grid-column': day + 3 }"
        />
        <div
          v-for="hour in hours"
          :key="`20${hour}`"
          class="cal__row"
          :style="{ 'grid-row': hour }"
        />
        <div
          v-if="isCurrentWeek"
          class="cal__current-time"
          :style="{
            'grid-column': currentDayOnGrid,
            'grid-row': currentHourOnGrid,
            top: currentMinute,
          }"
        >
          <div class="cal__circle" />
        </div>
      </div>
    </div>
    <CalendarNavigator class="cal__navigator" :month="currentMonth" />
  </div>
</template>

<script lang="ts">
import { inject } from "vue";

import { enUS } from "date-fns/locale";
import {
  getHours,
  getDay,
  getMinutes,
  getMonth,
  addDays,
  addWeeks,
  addSeconds,
  startOfWeek,
  format,
  Locale,
} from "date-fns";

import { range } from "lodash";

import CalendarNavigator from "@/components/CalendarNavigator.vue";
import { UserService } from "@/services/UserService";
import { DateService } from "@/services/DateService22";

import { Vue, Options, setup } from "vue-class-component";

@Options({
  components: {
    CalendarNavigator,
  },
})
export default class WorkCalendar extends Vue {
  dayFormatTokens = "do. MMMM yyyy";
  monthFormatTokens = "LLLL wo. 'tydzień' yyyy";

  hours = range(1, 24);
  days = range(0, 7);

  hourTime(number: number): string {
    if (number < 10) {
      return `0${number}:00`;
    }
    return `${number}:00`;
  }

  mounted(): void {
    setInterval(() => {
      this.pickedDate = addSeconds(this.pickedDate, 1);
    }, 1000);
  }

  dateService = setup(() => inject<DateService>("dateService")) as DateService;
  pickedDate = this.dateService.getNow();

  userService = setup(() => inject<UserService>("userService")) as UserService;
  locale: Locale = this.userService.user.preferences.locale ?? enUS;

  get isCurrentWeek(): boolean {
    const pickedDateMonday = startOfWeek(this.pickedDate, {
      locale: this.locale,
    });
    const currentMonday = startOfWeek(this.dateService.getNow(), {
      locale: this.locale,
    });

    return pickedDateMonday.getTime() === currentMonday.getTime();
  }

  setToday(): void {
    this.pickedDate = this.dateService.getNow();
  }

  addOneWeek(): void {
    this.pickedDate = addWeeks(this.pickedDate, 1);
  }

  subtractOneWeek(): void {
    this.pickedDate = addWeeks(this.pickedDate, -1);
  }

  getDayFromMonday(dayFromMonday: number): Date {
    const monday = startOfWeek(this.pickedDate, { locale: this.locale });
    return addDays(monday, dayFromMonday);
  }

  formatDate(date: Date, formatTokens: string): string {
    return format(date, formatTokens, { locale: this.locale });
  }

  get showMonth(): string {
    return this.formatDate(this.pickedDate, this.monthFormatTokens);
  }

  get currentDayOnGrid(): number {
    const gridAdjustment = 2;
    return getDay(this.pickedDate) + gridAdjustment;
  }

  get currentMonth(): number {
    return getMonth(this.pickedDate);
  }

  get currentHourOnGrid(): number {
    const gridAdjustment = 1;
    return getHours(this.pickedDate) + gridAdjustment;
  }

  get currentMinute(): string {
    const oneHourInMinutes = 60;
    const minutesPercentage =
      (getMinutes(this.pickedDate) / oneHourInMinutes) * 100;

    return `${Math.round(minutesPercentage)}%`;
  }
}
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
  &__container {
    width: 100%;
    display: grid;
    grid-template-rows: @title-height @days-height auto;
  }

  &__header {
    @apply bg-teal-500;
    text-align: center;
    display: flex;
    align-items: center;
    justify-content: space-between;
    z-index: 10;

    position: sticky;
    top: 4rem;
  }

  &__title {
    color: #fff;
    width: 20%;
    @apply text-white;
  }

  &__today {
    width: 10%;
    border-radius: 0 !important;
    height: 100%;
  }

  &__nav-icon {
    @apply h-6;
    @apply text-white;
    width: 25%;
  }

  &__days {
    @apply bg-gray-200;
    display: grid;
    place-content: center;
    text-align: center;
    grid-template-columns: @calendar-template;
    z-index: 10;
    border-bottom: 2px solid @grid-color;

    position: sticky;
    top: calc(4rem + @title-height);
  }

  &__day {
    border-left: 1px solid @grid-color;
  }

  &__content {
    display: grid;
    grid-template-columns: @calendar-template;
    grid-template-rows: repeat(24, @time-height);
  }

  &__time {
    grid-column: 1;
    text-align: right;
    align-self: end;
    font-size: 80%;
    position: relative;
    bottom: -0.5rem;
    color: #70757a;
    padding-right: 2px;
  }

  &__col {
    border-right: 1px solid @grid-color;
    grid-row: ~"1 / span 24";
    grid-column: span 1;
  }

  &__filler-col {
    grid-row: ~"1 / -1";
    grid-column: 2;
    border-right: 1px solid @grid-color;
  }

  &__row {
    grid-column: ~"2 / -1";
    border-bottom: 1px solid @grid-color;
  }

  &__current-time {
    border-top: 2px solid @current-time-color;
    position: relative;
  }

  &__circle {
    width: 12px;
    height: 12px;
    border: 1px solid @current-time-color;
    border-radius: 50%;
    background: @current-time-color;
    position: relative;
    top: -7px;
    left: -7px;
  }

  &__navigator {
    position: fixed;
    top: 500px;
    bottom: 33px;
    right: 20px;
  }
}
</style>
