using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Logic
{
    public static class CompatibilityChecker
    {
        /// <summary>
        /// Основной метод проверки всей сборки
        /// </summary>
        //public static string CheckAll(
        //    cpu_ cpu,
        //    motherboard_ mb,
        //    cooler_ cooler,
        //    gpu_ gpu,
        //    ram_ ram,
        //    case_ pcCase,
        //    psu_ psu)
        //{
        //    // 1. Совместимость сокета (Процессор + Мать + Кулер)
        //    // В вашем SQL: Таблицы cpu$, motherboard$, cooler$ все имеют поле [socketid]
        //    if (cpu != null && mb != null)
        //    {
        //        if (cpu.socketid != mb.socketid)
        //            return "❌ Процессор и материнская плата имеют разные сокеты.";
        //    }

        //    if (mb != null && cooler != null)
        //    {
        //        if (mb.socketid != cooler.socketid)
        //            return "❌ Кулер не поддерживает сокет материнской платы.";
        //    }

        //    // 2. Форм-фактор (Материнка + Корпус)
        //    // В вашем SQL: Поле называется [motherboardformfactorid] в обеих таблицах
        //    if (mb != null && pcCase != null)
        //    {
        //        if (mb.motherboardformfactorid != pcCase.motherboardformfactorid)
        //            return "❌ Форм-фактор материнской платы не подходит для этого корпуса.";
        //    }

        //    // 3. Тип памяти (Мать + ОЗУ)
        //    // В вашем SQL: Поле называется [ramtypeid] в обеих таблицах
        //    if (mb != null && ram != null)
        //    {
        //        if (mb.ramtypeid != ram.ramtypeid)
        //            return "❌ Тип памяти ОЗУ не поддерживается материнской платой.";
        //    }

        //    // 4. Мощность БП (БП + Видеокарта)
        //    // В вашем SQL: Таблица [psu$] имеет поле [power], а [gpu$] — [recommendedpsupower]
        //    if (gpu != null && psu != null)
        //    {
        //        if (psu.power < gpu.recommendedpsupower)
        //            return $"❌ Недостаточная мощность БП. Нужно минимум {gpu.recommendedpsupower}W.";
        //    }

        //    return "✅ Конфигурация совместима!";
        //}
    }
    
}
