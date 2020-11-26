const getNow = (): Date => {
    return new Date();
};

export interface DateService {
    getNow: () => Date;
}

export default { getNow } as DateService;
