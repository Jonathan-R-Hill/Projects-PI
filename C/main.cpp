#include <sys/ptrace.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <sys/user.h>
#include <unistd.h>
#include <iostream>

#define REG_PC 64 // Offset for PC (example only—check your platform!)

int main()
{
    pid_t child = 1053690;
    int status;
    while (true)
    {
        waitpid(child, &status, 0);
        if (WIFEXITED(status))
            break;

        // Read syscall number from ORIG_RAX
        long syscall = ptrace(PTRACE_PEEKUSER, child, sizeof(long) * REG_PC, nullptr);
        std::cout << "Syscall: " << syscall << std::endl;

        // Continue to next syscall
        ptrace(PTRACE_SYSCALL, child, nullptr, nullptr);
    }

    return 0;
}
