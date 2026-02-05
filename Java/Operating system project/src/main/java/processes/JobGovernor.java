package processes;

import core.Logger;
import core.Process;
import resources.*;

public class JobGovernor extends Process {

    private int step = 1;
    private Process vm;

    public JobGovernor() {
        this.pID = "JobGovernor";
        this.priority = 97;
    }

    @Override
    public void execute() {
        //Logger.log("Executing process: " + this);

        switch (step) {
            case 1:
                //Logger.log("[JobGovernor] Requesting user memory block");
                kernel.requestResource(this, "UserMemoryResource");
                step = 2;
                break;

            case 2:
                //Logger.log("[JobGovernor] Creating page table and loading program");
                kernel.requestResource(this, "PageTableResource");
                kernel.requestResource(this, "TaskInSupervMemoryResource");
                //  kodo perkėlimą iš supervizorinės atminties į vartotojo atmintį
                step = 3;
                break;

            case 3:
                //Logger.log("[JobGovernor] Creating VirtualMachine process");
                vm = new VirtualMachine();
                kernel.createProcess(this, vm);
                step = 4;
                break;

            case 4:
                //Logger.log("[JobGovernor] Blocking, waiting for interrupt");
                kernel.requestResource(this, "InterruptResource");
                step = 5;
                break;

            case 5:
                //Logger.log("[JobGovernor] Handling interrupt");
                // Čia s pertraukimų identifikavimą (GET/PUT/HALT)
                boolean isIO = false; // tarkim

                if (!isIO) {
                    //Logger.log("[JobGovernor] HALT or non-IO interrupt: releasing VM and memory");
                    kernel.destroyProcess(vm);
                    kernel.freeResource(this, new UserMemoryResource());
                    kernel.createResource(this, new ResultResource());
                    kernel.createResource(this, new TaskInSupervMemoryResource());
                    step = 6;
                } else {
                    //Logger.log("[JobGovernor] Handling IO (GET or PUT) interrupt");
                    // Simuliuoti GET arba PUT loginę resurso sąveiką
                    step = 4; // toliau laukti kitos pertraukos
                }
                break;

            case 6:
                //Logger.log("[JobGovernor] Blocking permanently, waiting for non-existing resource");
                kernel.requestResource(this, "NEEGZISTUOJANTIS_RESURSAS"); //gal reiktu sukurt o gal ne
                break;
        }
    }
}
