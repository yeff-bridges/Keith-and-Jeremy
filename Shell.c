/*
// 7/6/18  Let the shell shenanigans begin!
// 7/13/18 It runs! Includes reading and splitting lines.
// 7/15/18 Basic command execution and process forking added.
// 7/16/18 We're on github!
*/

#include <sys/wait.h>
#include <unistd.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#define SH_RL_BUFSIZE 1024
#define SH_SL_BUFSIZE 64
#define SH_TOK_DELIM " \t\r\n\v\f" //posix characters for whitespace

void sh_loop();
char *sh_read_line();
char **sh_split_line(char *line);
int sh_execute(char **args);
int sh_launch(char **args);
int sh_cd(char **args);
int sh_help(char **args);
int sh_pipe(char **args);
int sh_redirect(char **args);
int sh_exit(char **args);


//List of current built-in functions of the shell.
char *builtin_str[] = {
  	"cd",
  	"help",
  	"exit"
};

char *builtin_sym[] = { //structure: arg1 [SYMBOL] arg2 
	"|",		//pipe cmd1 stdout to cmd2
	"<",		//redirect arg2 into input of arg1
	">"		//redirect output of arg1 into input of arg2 
};

//Function pointers to built-in commands
int (*builtin_func[]) (char **) = {
	&sh_cd,
	&sh_help,
	&sh_exit
};

const unsigned int sh_num_builtins = sizeof(builtin_str) / sizeof(char);



//The shell begins here. We execute a main loop and then exit succesfully if we leave it.
int main(int argc, char **argv)
{
	sh_loop();
	return EXIT_SUCCESS;
}



//Here we have the main loop. In it, we first read in user input, split it into an array of arguments, and then punt
//it off for execution. This loop continues until the exit command is called. This command returns status == 0, 
//while any other returns status == 1.
void sh_loop() {
	char *line;
	char **args;
	int status;

  do {
	  char *CurDir = malloc(sizeof(char)*512);
	  getcwd(CurDir, sizeof(char)*512);
	  printf("%s> ", CurDir);
	  line = sh_read_line();
	  args = sh_split_line(line);
	  status = sh_execute(args);

	  free(line);
	  free(args);
	} while (status);
}



//Here we handle the task of reading in the user's input. We initialize a buffer, then loop indefinitely until an 
//end of line character is read. Then we return to the main loop with the filled buffer. If our initial buffer or 
//any later buffer is full, then we reallocate a new one that is larger before checking for more input. The code 
//could be shorter by using certain newer C commands, but you hide details.
char *sh_read_line() {
	unsigned int bufsize = SH_RL_BUFSIZE;
	unsigned int position = 0;
	char *line = malloc(sizeof(char) * bufsize);
	unsigned int c;

	if (!line) {
		fprintf(stderr, "sh: allocation error\n");
		exit(EXIT_FAILURE);
	}
	while(1) {
		c = getchar();	
	
		if (c == EOF||c == '\n') {
			line[position] = '\0';
			return line;
		} else {
			line[position] = c;
		}
		position++;
	
		if(position >= bufsize) {
			bufsize += SH_RL_BUFSIZE;
			line = realloc(line, bufsize);
			if(!line) {
				fprintf(stderr, "sh: allocation error\n");
				exit(EXIT_FAILURE);
			}
		}
	}
}



//This function splits the newly read in input and stores them in an array based on the whitespaces in the input. 
//Other delimiters will be added in the future.
char **sh_split_line(char *line) {
	unsigned int bufsize = SH_SL_BUFSIZE, position = 0;
	char **split_line = malloc(bufsize * sizeof(char*));
	char *token;

	if(!split_line) {
		fprintf(stderr, "sh: allocation error\n");
		exit(EXIT_FAILURE);
	}

	token = strtok(line, SH_TOK_DELIM);

	while(token) {
		
		split_line[position] = token;

		position++;

		if(position >= bufsize) {
			bufsize += SH_SL_BUFSIZE;
			split_line = realloc(split_line, bufsize * sizeof(char*));
			if(!split_line) {
				fprintf(stderr, "sh: allocation error\n");
				exit(EXIT_FAILURE);
			}
		}

		token = strtok(NULL, SH_TOK_DELIM);
	}

	split_line[position] = NULL;
	return split_line;
}

//This function checks if the command passed in args[0] is a built-in function. If so, we execute it.
//If not, we call the launcher to handle it.
int sh_execute(char **args) {
	if (!args[0]){
		return 1;
	}

	for (int i=0; i < sh_num_builtins; i++) {
		if(strcmp(args[0], builtin_str[i]) == 0) {
			return (*builtin_func[i])(args);
		}
	}

	return sh_launch(args);
}



//If the launcher is called, then we were passed a command we don't have built in, like a program in the directory. 
//We then fork to create a new process and use the execvp call to attempt to run the command. If it can't be run 
//for any reason, we print an error message.
int sh_launch(char **args) {
	pid_t pid, wpid;
	int status;

	if(pid > 0) { //parent stuff
		do {
			wpid = waitpid(pid, &status, WUNTRACED);
		} while (!WIFEXITED(status) && !WIFSIGNALED(status));
	} else if(pid == 0) { //child stuff
		if(execvp(args[0], args) == -1){
			perror("sh");
		}
		exit(EXIT_FAILURE);
	} else { //error forking
		perror("sh");
	}

	return 1;
}



//The following are the functions handling the built-in commands.
int sh_cd(char **args) {
	if(!args[1]) {
		fprintf(stderr, "Expected file path argument not found\n");
		return 1;
	} else {
		int status = chdir(args[1]);
		if(status) {
			perror("sh");
		}
	}

	return 1; 
}

int sh_help(char ** args){
	printf("Help is for bitches.\n");
	return 1;
}

int sh_pipe(char **args){
	printf("pipe.\n");
}

int sh_redirect(char **args){
	printf("redirect.\n");
}

int sh_exit(char **args){
	return 0;
}

